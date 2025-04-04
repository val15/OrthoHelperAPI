import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      return true;  // Autoriser l'accès à la route
    } else {
      this.router.navigate(['/login']);  // Rediriger vers la page de login
      return false;  // Bloquer l'accès à la route
    }
  }
}
