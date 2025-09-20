import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from './auth/login/login.component'; // 1. Importamos el "manual" de nuestra pieza

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, LoginComponent], // 2. Añadimos la pieza a la "caja de herramientas"
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'FinanceManager-UI';
}
