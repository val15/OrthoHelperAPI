import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root' // Permet à Angular d’injecter ce service automatiquement
})
export class TextService {
  private text: string = ''; // Texte original
  private correctedTextSubject: BehaviorSubject<string> = new BehaviorSubject<string>('');  // Sujet pour le texte corrigé

  setText(newText: string) {
    this.text = newText;
    //console.log('[TextService] Texte mis à jour :', this.text);
  }

  getText(): string {
    return this.text;
  }


  // Mettre à jour le texte corrigé
  setCorrectedText(newCorrectedText: string) {
    this.correctedTextSubject.next(newCorrectedText);  // Met à jour l'état et notifie les abonnés
  }

  // Récupérer un observable du texte corrigé
  getCorrectedText$() {
    return this.correctedTextSubject.asObservable();
  }
}
