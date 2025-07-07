import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

import { FormsModule } from '@angular/forms';

import { TextService } from './services/text.service';
import { ApiResponse } from './models/api-response.model';
import { AuthService } from './services/auth.service';

import { TextEditorComponent } from './components/text-editor/text-editor.component';
import { CorrectTextComponent } from './components/correct-text/correct-text.component';
import { environment } from '../environments/environment';
import { ModelsService } from './services/models.service';

const baseUrl = environment.apiBaseUrl;
//const baseUrl = 'http://localhost:8088'; //TEST ✅ URL de base ici
//const baseUrl = 'http://localhost:7088'; // PROD
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    TextEditorComponent,
    CorrectTextComponent,
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [ModelsService],
})
export class AppComponent implements OnInit {
  title = 'ortho-helper-angular-app';
  apiResponse: ApiResponse | null = null;
  errorMessage: string | null = null;

  models: string[] = [];
  selectedModel: string = '';

  activeTab: 'correct' | 'translate' = 'correct';

  constructor(
    private http: HttpClient,
    private textService: TextService,
    public authService: AuthService,
    private router: Router,
    private modelsService: ModelsService
  ) {
    this.textService.setCorrectedText('');
  }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/editor']);
    }
    const token = this.authService.getToken(); // Récupérer le token de l'utilisateur
    if (!token) {
      console.error('Token manquant — utilisateur non authentifié');
      return;
    }
  }

  loadModels(): void {
    this.modelsService.loadModels(); // Appel de la méthode du service

    // Souscription aux changements de modèles
    this.modelsService.models$.subscribe({
      next: (loadedModels) => {
        this.models = loadedModels;
        this.selectedModel = this.models[0] || '';
      },
      error: (error) => {
        console.error('Erreur lors du chargement des modèles:', error);
      },
    });
  }

  setTab(tab: 'correct' | 'translate') {
    this.activeTab = tab;
  }

  sendToApi() {
    const token = this.authService.getToken();
    if (!token) {
      console.error('Token non trouvé');
      return;
    }

    this.textService.setCorrectedText('Traitement en cours...');
    const textToSend = this.textService.getText();
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });

    // Choix de l'endpoint selon l'onglet actif
    const endpoint =
      this.activeTab === 'translate'
        ? '/api/Text/translate'
        : '/api/Text/correct';

    this.http
      .post<ApiResponse>(
        `${baseUrl}${endpoint}`,
        {
          text: textToSend,
          modelName: this.selectedModel,
        },
        { headers }
      )
      .subscribe({
        next: (response: ApiResponse) => {
          this.apiResponse = response;
          this.textService.setCorrectedText(this.apiResponse.outputText);
          this.errorMessage = null;
        },
        error: (error) => {
          this.apiResponse = null;
          this.errorMessage = error.message || 'Erreur inconnue';
          this.textService.setCorrectedText(`Erreur API: ${error.status}`);
        },
      });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
