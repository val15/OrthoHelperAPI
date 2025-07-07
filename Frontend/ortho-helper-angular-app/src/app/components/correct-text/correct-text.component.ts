import { Component, OnInit, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TextService } from '../../services/text.service'; // Importer TextService

@Component({
  selector: 'app-correct-text',
  standalone: true,
  // imports: [],
  imports: [FormsModule],
  templateUrl: './correct-text.component.html',
  styleUrls: ['./correct-text.component.css'],
})
//export class ApiResponseComponent {
//  apiResponse: string = '';
//}
export class CorrectTextComponent implements OnInit {
  @Input() activeTab: 'correct' | 'translate' = 'correct';
  correctedText: string = '';

  constructor(private textService: TextService) {}

  onTextChange() {
    console.log('CorrectText:', this.correctedText);
    // this.textService.setText(this.userText); // Sauvegarde du texte
  }
  ngOnInit() {
    // Lors de l'initialisation du composant, récupérer le texte corrigé depuis le service
    // S'abonner à l'observable pour recevoir le texte corrigé à chaque mise à jour
    this.textService.getCorrectedText$().subscribe((correctedText) => {
      this.correctedText = correctedText;
    });
  }

  userText: string = '';

  copyCorrectedText(): void {
    if (!this.correctedText) {
      console.warn('Aucun texte à copier.');
      return;
    }

    navigator.clipboard
      .writeText(this.correctedText)
      .then(() => {
        console.log('Texte corrigé copié dans le presse-papiers.');
      })
      .catch((err) => {
        console.error('Erreur lors de la copie :', err);
      });
  }
}
