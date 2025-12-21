// src/app/services/api.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

// const API_BASE = '/api'; // adjust if your backend base path differs
const API_BASE = '/api';


export interface Project {
  id: number;
  name: string;
  description?: string;
}

export interface TaskItem {
  id: number;
  title: string;
  isCompleted: boolean;
  projectId: number;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) {}

  // Safe localStorage access: works in browser, returns null during server-side prerender
  private getToken(): string | null {
    try {
      if (typeof window === 'undefined' || !window.localStorage) return null;
      return localStorage.getItem('token');
    } catch {
      // defensive fallback
      return null;
    }
  }

  // Only attach Authorization header if token available
  private authOptions() {
    const token = this.getToken();
    if (!token) return {};
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    return { headers };
  }

  // Store token only when running in browser
  private storeToken(token: string | null) {
    try {
      if (typeof window === 'undefined' || !window.localStorage) return;
      if (token) localStorage.setItem('token', token);
      else localStorage.removeItem('token');
    } catch {
      // ignore storage failures
    }
  }

  // Auth endpoints
    // Auth endpoints (backend expects "username" + "password")
  register(username: string, password: string): Observable<any> {
    return this.http.post(`${API_BASE}/Auth/register`, { username, password });
  }

    login(username: string, password: string): Observable<any> {
    const body = { username, password };

    // TEMP DEBUG: confirm what Angular is sending
    console.log('LOGIN BODY being sent:', body);

    return this.http.post<{ token: string }>(`${API_BASE}/Auth/login`, body)
      .pipe(
        tap(resp => {
          const tok = resp?.token;
          if (typeof tok === 'string') this.storeToken(tok);
        })
      );
  }



  logout() {
    this.storeToken(null);
  }

  // Projects
  getProjects(): Observable<Project[]> {
    return this.http.get<Project[]>(`${API_BASE}/Projects`, this.authOptions());
  }

  createProject(p: { name: string; description?: string }) {
    return this.http.post<Project>(`${API_BASE}/Projects`, p, this.authOptions());
  }

  // Tasks
  // Return all tasks (used by TasksComponent.getTasks())
  getTasks(): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(`${API_BASE}/Tasks`, this.authOptions());
  }

  // Return tasks for a specific project (if you need this elsewhere)
  getTasksForProject(projectId: number): Observable<TaskItem[]> {
    return this.http.get<TaskItem[]>(`${API_BASE}/Tasks/project/${projectId}`, this.authOptions());
  }

  createTask(t: { title: string; projectId: number; isCompleted?: boolean }) {
    // backend expects isCompleted (boolean). default false.
    const body = {
      title: t.title,
      projectId: t.projectId,
      isCompleted: t.isCompleted ?? false
    };

    return this.http.post<TaskItem>(`${API_BASE}/Tasks`, body, this.authOptions());
  }

}
