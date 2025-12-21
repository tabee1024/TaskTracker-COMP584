import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../services/api';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html'
})
export class RegisterComponent {
  username = '';
  password = '';
  confirmPassword = '';
  error = '';
  success = '';

  constructor(private api: ApiService, private router: Router) {}

  register() {
    this.error = '';
    this.success = '';

    if (!this.username.trim() || !this.password) {
      this.error = 'Username and password are required.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }

    this.api.register(this.username.trim(), this.password).subscribe({
      next: () => {
        this.success = 'Account created! You can now log in.';
        // Optional: auto-send them to login after 1 sec
        setTimeout(() => this.router.navigate(['/login']), 800);
      },
      error: (err) => {
        // Backend returns Identity errors sometimes as array
        const body = err?.error;
        if (Array.isArray(body)) {
          this.error = body.map((e: any) => e.description ?? e.code ?? 'Error').join(' | ');
        } else if (typeof body === 'string') {
          this.error = body;
        } else {
          this.error = 'Registration failed.';
        }
        console.error('Register error:', err);
      }
    });
  }
}
