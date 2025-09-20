import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms'; // Necesario para ngModel
import { CommonModule } from '@angular/common'; // Necesario para directivas como ngIf, ngFor, etc.

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule], // Importamos los módulos que usaremos
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  // Creamos un objeto para guardar los datos del formulario.
  model: any = {};

  constructor() { }

  // Este método se ejecutará cuando el usuario envíe el formulario.
  login() {
    // Por ahora, solo mostraremos los datos en la consola para probar.
    console.log("Datos del formulario:", this.model);
  }
}

