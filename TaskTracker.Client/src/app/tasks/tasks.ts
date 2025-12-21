// src/app/tasks/tasks.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class TasksComponent implements OnInit {
  taskForm!: FormGroup;

  projects: any[] = [];
  tasks: any[] = [];

  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private api: ApiService,
    private router: Router
  ) {}

  ngOnInit() {
    this.taskForm = this.fb.group({
      title: ['', Validators.required],
      projectId: [null, Validators.required]
    });

    if (typeof window !== 'undefined') {
      this.loadProjects();
      this.loadTasks();
    }
  }

  logout() {
    this.api.logout();          // clears token
    this.router.navigate(['/login']);
  }

  loadProjects() {
    this.api.getProjects().subscribe({
      next: (res: any) => (this.projects = res || []),
      error: () => (this.error = 'Failed to load projects')
    });
  }

  loadTasks() {
    this.api.getTasks().subscribe({
      next: (res: any) => (this.tasks = res || []),
      error: () => (this.error = 'Failed to load tasks')
    });
  }

  createTask() {
    if (this.taskForm.invalid) return;

    this.loading = true;
    this.error = '';

    const formValue = this.taskForm.value;

    const payload = {
      title: formValue.title,
      projectId: Number(formValue.projectId)
    };

    this.api.createTask(payload).subscribe({
      next: () => {
        this.taskForm.reset();
        this.loadTasks();
        this.loading = false;
      },
      error: () => {
        this.error = 'Create failed';
        this.loading = false;
      }
    });
  }
}
