import {Routes} from '@angular/router';

export const BillingRoutes: Routes = [
  {
    path: '',
    redirectTo: 'invoice-notes',
    pathMatch: 'full'
  },

  {
    path: 'invoice-notes',
    loadComponent: () => import('./invoice-notes-list/invoice-notes-list.component')
      .then(m => m.InvoiceNotesListComponent)
  },
]
