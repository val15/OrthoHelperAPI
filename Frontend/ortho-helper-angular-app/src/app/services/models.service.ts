import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { BehaviorSubject, map, Observable } from 'rxjs';
const baseUrl = environment.apiBaseUrl;
@Injectable({
  providedIn: 'root',
})
export class ModelsService {
  private models = new BehaviorSubject<string[]>([]);
  public models$ = this.models.asObservable();

  constructor(private http: HttpClient, private authService: AuthService) {}

  loadModels(): void {
    const token = this.authService.getToken();
    if (!token) return;

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    this.http
      .get<any[]>(`${environment.apiBaseUrl}/api/TextCorrection/models`, {
        headers,
      })
      .subscribe({
        next: (response) => {
          this.models.next(response.map((m) => m.modelName));
        },
        error: (err) => {
          console.error('Erreur de chargement des modèles :', err);
        },
      });
  }
}
