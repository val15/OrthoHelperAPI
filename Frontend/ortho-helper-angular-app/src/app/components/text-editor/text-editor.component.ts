import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TextService } from '../../services/text.service';

@Component({
  selector: 'app-text-editor',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './text-editor.component.html',
  styleUrls: ['./text-editor.component.css']
})
export class TextEditorComponent {
  userText: string = '';

  constructor(private textService: TextService) { }

  onTextChange() {
    //console.log('Texte modifié:', this.userText);
    this.textService.setText(this.userText); // Sauvegarde du texte
  }

   
  

  pasteText() {
    navigator.clipboard.readText().then(text => {
      this.userText = text;
    }).catch(err => {
      console.error('Erreur lors du collage :', err);
    });
  }
}
