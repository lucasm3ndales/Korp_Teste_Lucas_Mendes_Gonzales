import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {SidenavComponent} from './shared/components/sidenav/sidenav.component';
import {ButtonModule} from 'primeng/button';
import {CommonModule} from '@angular/common';
import {ToastModule} from 'primeng/toast';


@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    SidenavComponent,
    ButtonModule,
    CommonModule,
    ToastModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  sidenavVisible: boolean = false;
}
