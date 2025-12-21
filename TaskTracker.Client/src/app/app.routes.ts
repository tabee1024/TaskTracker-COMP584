// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { LoginComponent } from './login/login';
import { ProjectsComponent } from './projects/projects';
import { TasksComponent } from './tasks/tasks';
import { authGuard } from './guards/auth.guard';
import { RegisterComponent } from './register/register';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Protected routes
  { path: 'projects', component: ProjectsComponent, canActivate: [authGuard] },
  { path: 'tasks', component: TasksComponent, canActivate: [authGuard] },

  { path: '**', redirectTo: 'login' }
];
