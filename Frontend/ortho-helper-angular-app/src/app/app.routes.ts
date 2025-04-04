import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
// import { RegisterComponent } from './components/register/register.component';
import { TextEditorComponent } from './components/text-editor/text-editor.component';
import { CorrectTextComponent } from './components/correct-text/correct-text.component';
import { AuthGuard } from './guards/auth.guard'; // Importez la garde de route

// Définition des routes
export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },  // Route par défaut vers login
  { path: 'login', component: LoginComponent },
  // { path: 'register', component: RegisterComponent },

  // Routes protégées par AuthGuard
  {
    path: 'editor',
    component: TextEditorComponent,
    canActivate: [AuthGuard]  // Protéger l'accès à la route "editor"
  },
  {
    path: 'correct',
    component: CorrectTextComponent,
    canActivate: [AuthGuard]  // Protéger l'accès à la route "correct"
  },

  { path: '**', redirectTo: 'login' }  // Redirige vers login en cas de route inconnue
];
