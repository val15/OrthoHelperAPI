import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Pour les formulaires template-driven
import { HttpClient, HttpHeaders } from '@angular/common/http'; // Pour les requêtes HTTP
import { Router } from '@angular/router'; // Pour la navigation
import { AuthService } from '../../services/auth.service'; // Importez AuthService
import { environment } from '../../../environments/environment';

const baseUrl = environment.apiBaseUrl;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule], // Importez FormsModule ici
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = ''; // Variable pour stocker le nom d'utilisateur
  password: string = ''; // Variable pour stocker le mot de passe
  errorMessage: string | null = null; // Variable pour afficher les messages d'erreur

  constructor(
    private http: HttpClient,
    private router: Router,
    private authService: AuthService // Injectez AuthService
  ) {}

  onSubmit(): void {
    // URL de l'API de login
    //const apiUrl = 'https://localhost:32774/api/auth/login'; //DEV Remplacez par l'URL de votre API

    const apiUrl = `${baseUrl}/api/auth/login`
    //const apiUrl = 'http://localhost:8088/api/auth/login'; // TEST 
    //const apiUrl = 'http://localhost:7088/api/auth/login'; // PROD

    // Données à envoyer (nom d'utilisateur et mot de passe)
    const loginData = {
      username: this.username,
      password: this.password
    };

    // En-têtes de la requête
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    // Envoyer la requête POST à l'API
    this.http.post<{ token: string }>(apiUrl, loginData, { headers: headers })
      .subscribe({
        next: (response) => {
          // Si la connexion réussit, stockez le token et marquez l'utilisateur comme connecté
          if (response.token) {
            this.authService.login(response.token); // Utilisez AuthService pour gérer la connexion
          } else {
            this.errorMessage = 'Token non reçu du serveur';
            console.error('Erreur : Token non reçu');
          }
        },
        error: (error) => {
          // En cas d'erreur, affichez un message d'erreur
          this.errorMessage = error.error.message || 'Échec de la connexion';
          console.error('Erreur de connexion:', error);
        }
      });
  }
}
