import { Component, Input, Output, EventEmitter } from '@angular/core';
import {CommonModule, NgOptimizedImage} from '@angular/common';
import {RouterLink, RouterLinkActive} from '@angular/router';
import { ButtonModule } from 'primeng/button';
import {DrawerModule} from 'primeng/drawer';
import {IMenuItems} from './models/sidenav.model';


@Component({
  selector: 'app-sidenav',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    ButtonModule,
    DrawerModule,
    RouterLinkActive,
    NgOptimizedImage
  ],
  templateUrl: './sidenav.component.html',
  styleUrl: './sidenav.component.css',
})
export class SidenavComponent {
  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();
  logo: string = '/assets/images/logo-full.jpeg'

  menuItems: IMenuItems[] = [
    { label: 'Faturamento', icon: 'pi pi-dollar', link: '/billing' },
    { label: 'Estoque', icon: 'pi pi-box', link: '/stock' }
  ];
  toggleSidebar() {
    this.visible = !this.visible;
    this.visibleChange.emit(this.visible);
  }
}
