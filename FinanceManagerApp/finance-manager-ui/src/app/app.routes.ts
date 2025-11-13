import { Routes } from '@angular/router';

export const routes: Routes = [
  
  {
    path: 'auth', // Cuando la URL empiece con 'auth'...
    children: [
      {
        path: 'login', // ...y siga con 'login' (/auth/login)
        loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register', // ...y siga con 'register' (/auth/register)
        loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent)
      },
      {
        path: '', // Si solo entran a /auth
        redirectTo: 'login', // Redirígelos a /auth/login
        pathMatch: 'full'
      }
    ]
  }
  // Aquí pueden ir otras rutas 
];
