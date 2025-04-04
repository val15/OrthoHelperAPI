import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common'; // Ajout de CommonModule
import { Router, RouterModule } from '@angular/router'; // Ajout de Router pour la navigation
import { routes } from './app.routes'; // Importer les routes depuis app.routes.ts

import { TextService } from './services/text.service';
import { ApiResponse } from './models/api-response.model';
import { AuthService } from './services/auth.service'; // Importez AuthService

import { TextEditorComponent } from './components/text-editor/text-editor.component';
import { CorrectTextComponent } from './components/correct-text/correct-text.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, TextEditorComponent, CorrectTextComponent], // Ajout de CommonModule et RouterModule
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'ortho-helper-angular-app';
  apiResponse: ApiResponse | null = null;
  errorMessage: string | null = null;

  constructor(
    private http: HttpClient, 
    private textService: TextService,
    public authService: AuthService, // Rendre authService publique
    private router: Router // Injectez Router pour la navigation
  ) {

    this.textService.setCorrectedText('');
   }

  // Méthode pour envoyer du texte à l'API
  sendToApi() {
    const token = this.authService.getToken(); // Récupérez le token depuis AuthService
    if (!token) {
      console.error('Token non trouvé');
      return;
    }

    this.textService.setCorrectedText('Traitement en cours...');
    console.log('Envoi à l\'API...');

    const textToSend = this.textService.getText();
    console.log('Texte envoyé :', textToSend);

    const headers = new HttpHeaders({ 'Authorization': `Bearer ${token}` });
    //const apiUrl = 'https://localhost:32768/api/Text/process'; //DEV
    const apiUrl = 'http://localhost:7088/api/TextCorrection/correct'; //TEST

    this.http.post<ApiResponse>(apiUrl, { text: textToSend }, { headers: headers })
      .subscribe({
        next: (response: ApiResponse) => {
          
          this.apiResponse = response;
          this.textService.setCorrectedText(this.apiResponse.outputText);
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

  // Méthode pour déconnecter l'utilisateur
  logout(): void {
    this.authService.logout(); // Appelez la méthode logout de AuthService
    this.router.navigate(['/login']); // Redirigez vers la page de login
  }
}
