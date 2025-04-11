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
const baseUrl = environment.apiBaseUrl;
//const baseUrl = 'http://localhost:8088'; //TEST ✅ URL de base ici
//const baseUrl = 'http://localhost:7088'; // PROD
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TextEditorComponent, CorrectTextComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'ortho-helper-angular-app';
  apiResponse: ApiResponse | null = null;
  errorMessage: string | null = null;

  models: string[] = [];
  selectedModel: string = '';

  constructor(
    private http: HttpClient,
    private textService: TextService,
    public authService: AuthService,
    private router: Router
  ) {
    this.textService.setCorrectedText('');
  }

  ngOnInit(): void {
    const token = this.authService.getToken(); // Récupérer le token de l'utilisateur
    if (!token) {
      console.error('Token manquant — utilisateur non authentifié');
      return;
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    this.http.get<any[]>(`${baseUrl}/api/TextCorrection/models`, { headers })
      .subscribe({
        next: (response) => {
          this.models = response.map(m => m.modelName);
          this.selectedModel = this.models[0] || '';
        },
        error: (err) => {
          console.error('Erreur de chargement des modèles :', err);
        }
      });
  }

  sendToApi() {
    const token = this.authService.getToken();
    if (!token) {
      console.error('Token non trouvé');
      return;
    }

    this.textService.setCorrectedText('Traitement en cours...');
    const textToSend = this.textService.getText();
    console.log('Texte envoyé :', textToSend);
    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });

    this.http.post<ApiResponse>(`${baseUrl}/api/TextCorrection/correct`, {
      text: textToSend,
      modelName: this.selectedModel
    }, { headers })
      .subscribe({
        next: (response: ApiResponse) => {
          this.apiResponse = response;
          this.textService.setCorrectedText(this.apiResponse.outputText);
          this.errorMessage = null;
          this.errorMessage = null;
          console.log('apiResponse :');
          console.log('outputText : ', this.apiResponse.outputText);
          console.log('processingTime : ', this.apiResponse.processingTime);
        },
        error: (error) => {
          this.apiResponse = null;
          this.errorMessage = error.message || 'Erreur inconnue';
          console.error('Erreur API:', error);
          this.textService.setCorrectedText(`Erreur API: ${error.status}`);
        }
      });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
