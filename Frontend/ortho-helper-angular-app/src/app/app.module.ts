import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';  // Importer FormsModule pour ngModel
import { HttpClientModule } from '@angular/common/http';  // Pour les requêtes HTTP
import { RouterModule } from '@angular/router';  // Assurez-vous que RouterModule est importé
import { routes } from './app.routes';  // Importer les routes

import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    // Ajoute tes autres composants ici
  ],
  imports: [
    BrowserModule,
    FormsModule,  // Importer FormsModule pour ngModel
    HttpClientModule,  // Importer HttpClientModule pour les appels API
    RouterModule.forRoot(routes),  // Assurez-vous d'utiliser forRoot() pour les routes
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
