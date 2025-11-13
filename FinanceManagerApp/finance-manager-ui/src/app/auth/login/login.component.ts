import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // <-- 1. IMPORTAR FormsModule
import { AuthService } from '../../core/services/auth.service'; // <-- 1. IMPORTA EL SERVICIO
import { response } from 'express';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  model: any = {}; 

  // Inyectando el contructor
  constructor(private authService: AuthService) { }

  //  función 'login' 
  login() { 
    console.log('Enviando al backend:', this.model);

    this.authService.login(this.model).subscribe({
      // 'next' se ejecuta si la llamada es exitosa
      next: (response) => {
        console.log('Respuesta exitosa', response);
      },

      // 'error' se ejecuta si la API devuelve un error
      error : (err) => {
        console.log('Hubo un error en el login', err)
      }
    })
  }
}
