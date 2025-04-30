import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isLoggedIn = false; // État de connexion

  constructor(private router: Router) {}

  // Méthode pour stocker le token et marquer l'utilisateur comme connecté
  login(token: string): void {
    localStorage.setItem('token', token); // Stockez le token dans le localStorage
    this.isLoggedIn = true;
    this.router.navigate(['/editor']); // Redirigez vers la page principale
  }

  // Méthode pour déconnecter l'utilisateur
  logout(): void {
    localStorage.removeItem('token'); // Supprimez le token du localStorage
    this.isLoggedIn = false;
    this.router.navigate(['/login']); // Redirigez vers la page de login
  }

  // Méthode pour vérifier si l'utilisateur est connecté
  isAuthenticated(): boolean {
    return this.isLoggedIn || !!localStorage.getItem('token'); // Vérifiez l'état de connexion
  }

  // Méthode pour récupérer le token
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
