import {Component, OnInit} from '@angular/core';
import {catchError, interval, merge, Observable, of, startWith, Subject, switchMap, take} from 'rxjs';
import {IColumn} from '../../../shared/components/table/models/table.model';
import {formatDate} from '../../../shared/utils/date.utils';
import {IInvoiceNote, InvoiceNoteStatus, translateInvoiceNoteStatus} from '../../../shared/models/invoice-note.model';
import {IApiResult} from '../../../shared/models/default.model';
import {ToastService} from '../../../core/services/snackbar/toast.service';
import {BillingService} from '../../../core/services/billing/billing.service';
import {AsyncPipe} from '@angular/common';
import {Button} from 'primeng/button';
import {TableComponent} from '../../../shared/components/table/table.component';
import {
  CreateInvoiceNoteDialogComponent
} from './components/create-invoice-note-dialog/create-invoice-note-dialog.component';
import {
  InvoiceNoteDetailsDialogComponent
} from './components/invoice-note-details-dialog/invoice-note-details-dialog.component';
import {Chip} from 'primeng/chip';

@Component({
  selector: 'app-invoice-notes-list',
  imports: [
    AsyncPipe,
    Button,
    TableComponent,
    CreateInvoiceNoteDialogComponent,
    InvoiceNoteDetailsDialogComponent,
    Chip
  ],
  templateUrl: './invoice-notes-list.component.html',
  styleUrl: './invoice-notes-list.component.css',
})
export class InvoiceNotesListComponent implements OnInit {
  private readonly POLLING_INTERVAL = 30000;
  readonly InvoiceNoteStatus = InvoiceNoteStatus;
  private refreshTrigger$ = new Subject<void>();

  invoiceNotes$!: Observable<IApiResult<IInvoiceNote[]>>;
  loading = false;

  showCreateDialog = false;
  showDetailsDialog = false;
  selectedInvoiceNoteId: string | null = null;

  readonly COLUMNS: IColumn[] = [
    {
      header: "Número Serial",
      field: "noteNumber",
    },
    {
      header: "Status",
      field: "status",
      render: (status: InvoiceNoteStatus) => translateInvoiceNoteStatus(status),
    },
    {
      header: "Criada Em",
      field: "createdAt",
      render: (createdAt: string) => formatDate(createdAt),
    },
    {
      header: "Atualizada Em",
      field: "updatedAt",
      render: (updatedAt: string) => formatDate(updatedAt),
    },
    {
      header: "Detalhes",
      field: "id",
    }
  ]

  constructor(
    private toastService: ToastService,
    private billingService: BillingService,
  ) {
  }

  ngOnInit(): void {
    const polling$ = interval(this.POLLING_INTERVAL);

    this.invoiceNotes$ = merge(polling$, this.refreshTrigger$).pipe(
      startWith(0),
      switchMap(() => this.fetchInvoiceNotes())
    );
  }

  private fetchInvoiceNotes(): Observable<IApiResult<IInvoiceNote[]>> {
    this.loading = true;
    return this.billingService.getAllInvoiceNotes().pipe(
      catchError(err => {
        console.error('Falha ao buscar notas fiscais:', err);
        const errorBody = err.error as IApiResult<any>
        this.toastService.showApiError(errorBody, 'Falha ao buscar notas fiscais!')
        return of({ isSuccess: false, messages: [], data: [] });
      }),
      switchMap(result => {
        this.loading = false;
        return of(result);
      })
    );
  }

  refreshInvoiceNotes(): void {
    this.refreshTrigger$.next();
  }

  openCreateInvoiceNoteDialog(): void {
    this.showCreateDialog = true;
  }

  closeCreateInvoiceNoteDialog(saved: boolean = false): void {
    this.showCreateDialog = false;
    if (saved) this.refreshInvoiceNotes();
  }

  openInvoiceNoteDetailsDialog(id: string): void {
    this.selectedInvoiceNoteId = id;
    this.showDetailsDialog = true;
  }

  closeInvoiceNoteDetailsDialog(): void {
    this.selectedInvoiceNoteId = null;
    this.showDetailsDialog = false;
  }
}
