// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <nav style="margin-bottom: 20px;">
      <a routerLink="/login">Login</a> |
      <a routerLink="/projects">Projects</a> |
      <a routerLink="/tasks">Tasks</a>
    </nav>

    <router-outlet></router-outlet>
  `
})
export class AppComponent {}
