import {Routes} from '@angular/router';

export const routes: Routes = [
  {path: '', pathMatch: 'full', redirectTo: '/billing'},
  {path: 'billing', loadChildren: () => import('./features/billing/billing.routes').then(m => m.BillingRoutes)},
  {path: 'stock', loadChildren: () => import('./features/stock/stock.routes').then(m => m.StockRoutes)},
  { path: '**', redirectTo: '/billing' }
];
