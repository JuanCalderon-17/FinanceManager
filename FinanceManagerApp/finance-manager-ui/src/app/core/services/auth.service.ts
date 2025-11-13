import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
 
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7133/api/';
  constructor(private http: HttpClient) { }

  // Creamos metodo login
  public login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model)
  } 

  public register(model: any) {
    // model será {username, email, password}
    // Esto llama a tu endpoint POST api/Account/register
    return this.http.post(this.baseUrl + 'account/register', model);
  }
}
