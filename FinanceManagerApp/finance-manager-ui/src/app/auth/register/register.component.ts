import { Component  } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { AuthService } from "../../core/services/auth.service";
import { response } from "express";
import { concatWith } from "rxjs";

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

export class RegisterComponent {
  model: any = {} 

  constructor(private authService: AuthService) { }
  
  //Añado funcion para llamar servicio
  register() {
    console.log('Enviando datos del registro:', this.model);  

    this.authService.register(this.model).subscribe({
      // 'next' se ejecuta si el registro es exitoso
      next: (response) => {
        console.log('Usuario registrado exitosamente', response)
        // 'response' aquí debería ser tu UserDto (username y token)
      },
      // 'error' se ejecuta si la API devuelve un error, ej: el username ya existe, el email es inválido, etc.
      error: (err) => {
        console.error("Error durante el registro mijo", err);
      }
    });
  }
} 