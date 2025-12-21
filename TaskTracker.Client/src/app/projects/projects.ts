// src/app/projects/projects.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class ProjectsComponent implements OnInit {
  projectForm!: FormGroup;
  projects: any[] = [];
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    this.projectForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });

    if (typeof window !== 'undefined') {
      this.loadProjects();
    }
  }

  logout() {
    this.api.logout();          // clears token
    this.router.navigate(['/login']);
  }

  loadProjects() {
    this.api.getProjects().subscribe({
      next: (res: any) => this.projects = res || [],
      error: () => this.error = 'Failed to load projects'
    });
  }

  createProject() {
    if (this.projectForm.invalid) return;

    this.loading = true;
    this.error = '';

    this.api.createProject(this.projectForm.value).subscribe({
      next: () => {
        this.projectForm.reset();
        this.loadProjects();
        this.loading = false;
      },
      error: () => {
        this.error = 'Create failed';
        this.loading = false;
      }
    });
  }
}
