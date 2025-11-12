import {Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges} from '@angular/core';
import {ICloseInvoiceNote, IInvoiceNote, InvoiceNoteStatus} from '../../../../../shared/models/invoice-note.model';
import {BillingService} from '../../../../../core/services/billing/billing.service';
import {ToastService} from '../../../../../core/services/snackbar/toast.service';
import {formatDate} from '../../../../../shared/utils/date.utils';
import {ChipModule} from 'primeng/chip';
import {DialogModule} from 'primeng/dialog';
import {ButtonModule} from 'primeng/button';
import {BehaviorSubject, finalize, Observable, of, Subject, switchMap, takeUntil} from 'rxjs';
import {IApiResult} from '../../../../../shared/models/default.model';
import {AsyncPipe, CommonModule} from '@angular/common';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {ICloseInvoiceNoteControls} from './models/close-invoice-note-form.model';

@Component({
  selector: 'app-invoice-note-details-dialog',
  standalone: true,
  imports: [
    DialogModule,
    ChipModule,
    ButtonModule,
    AsyncPipe,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './invoice-note-details-dialog.component.html',
  styleUrl: './invoice-note-details-dialog.component.css',
})
export class InvoiceNoteDetailsDialogComponent implements OnInit, OnChanges, OnDestroy {
  @Input() isOpen: boolean = false;
  @Input() invoiceNoteId: string | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() invoiceNoteClosed = new EventEmitter<void>();

  private invoiceNoteId$ = new Subject<string | null>();

  private invoiceNoteSub = new BehaviorSubject<IApiResult<IInvoiceNote> | null>(null);
  public invoiceNote$: Observable<IApiResult<IInvoiceNote> | null> = this.invoiceNoteSub.asObservable();

  public closeInvoiceNoteForm!: FormGroup<ICloseInvoiceNoteControls>;
  public isClosing = false;
  private destroy$ = new Subject<void>();

  public isLoading = false;
  public readonly InvoiceNoteStatus = InvoiceNoteStatus;
  public readonly formatDate = formatDate;


  constructor(
    private readonly billingService: BillingService,
    private readonly toastService: ToastService,
    private readonly fb: FormBuilder,
  ) {
    this.fetchInvoiceNoteDetails();
  }

  ngOnInit(): void {
    this.initForm();
    this.fetchInvoiceNoteDetails();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['invoiceNoteId']) {
      this.invoiceNoteId$.next(this.invoiceNoteId);
      if (this.closeInvoiceNoteForm) {
        this.closeInvoiceNoteForm.controls.id.setValue(this.invoiceNoteId);
      }
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initForm(): void {
    this.closeInvoiceNoteForm = this.fb.group<ICloseInvoiceNoteControls>({
      id: new FormControl(null, {
        nonNullable: false,
        validators: [Validators.required],
      }),
      rowVersion: new FormControl(null, {
        validators: [Validators.required],
      })
    });
  }

  private fetchInvoiceNoteDetails(): void {
    this.invoiceNoteId$
      .pipe(
        takeUntil(this.destroy$),
        switchMap(id => {
          if (!id) {
            this.invoiceNoteSub.next(null);
            return of(null);
          }

          this.isLoading = true;
          this.invoiceNoteSub.next(null);
          this.closeInvoiceNoteForm.controls.id.setValue(id);

          return this.billingService.getInvoiceNoteById(id)
            .pipe(
              finalize(() => (this.isLoading = false))
            );
        })
      )
      .subscribe({
        next: (result) => {
          if (result && result.isSuccess && result.data) {

            const rowVersion = result.data.rowVersion;

            if (rowVersion) {
              this.closeInvoiceNoteForm.controls.rowVersion.setValue(rowVersion);
            }

            this.invoiceNoteSub.next(result);

          } else if (result) {
            this.invoiceNoteSub.next(null);
          }
        },
        error: (err) => {
          console.error('Erro ao buscar detalhes da nota fiscal:', err);
          const errorBody = err.error as IApiResult<any>
          this.toastService.showApiError(errorBody, 'Falha ao buscar detalhes da nota fiscal.');
          this.invoiceNoteSub.next(null);
        }
      });
  }

  public closeInvoiceNote(): void {
    this.closeInvoiceNoteForm.markAllAsTouched();

    if (this.closeInvoiceNoteForm.invalid) {
      this.toastService.showError('Atenção', 'ID da nota fiscal não pode ser nulo.');
      return;
    }

    this.isClosing = true;
    const data: ICloseInvoiceNote = this.closeInvoiceNoteForm.getRawValue() as ICloseInvoiceNote;

    this.billingService.closeInvoiceNote(data)
      .pipe(finalize(() => this.isClosing = false))
      .subscribe({
        next: (result) => {
          if (result.isSuccess) {
            this.toastService.showApiSuccess(result, 'Nota fiscal fechada com sucesso!')
            this.invoiceNoteClosed.emit();
            this.onClose();
          } else {
            this.toastService.showApiError(result, 'Falha ao fechar nota fiscal.');
          }
        },
        error: (err) => {
          console.error('Erro ao fechar nota fiscal:', err);
          const errorBody = err.error as IApiResult<any>;
          this.toastService.showApiError(errorBody, 'Falha ao fechar nota fiscal.');
        }
      });
  }

  onClose(): void {
    this.invoiceNoteSub.next(null);
    this.isLoading = false;
    this.isClosing = false;
    this.close.emit();
  }
}
