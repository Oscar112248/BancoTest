import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from './layout/sidebar.component';
import { HeaderComponent } from './layout/header.component';
import { ToastContainerComponent } from "./toast-container.component";

@Component({
  selector: 'app-root',
  standalone: true,                                  // 👈 obligatorio
  imports: [RouterOutlet, SidebarComponent, HeaderComponent, ToastContainerComponent], // 👈 aquí importas tus hijos
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'banco-app';
    collapsed = false;   

}