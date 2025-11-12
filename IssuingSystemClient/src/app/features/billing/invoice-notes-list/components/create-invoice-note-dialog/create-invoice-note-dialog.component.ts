import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {
  FormArray,
  FormGroup,
  FormsModule,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {StockService} from '../../../../../core/services/stock/stock.service';
import {ToastService} from '../../../../../core/services/snackbar/toast.service';
import {IProduct} from '../../../../../shared/models/product.model';
import {IApiResult} from '../../../../../shared/models/default.model';
import {BillingService} from '../../../../../core/services/billing/billing.service';
import {ICreateInvoiceNoteControls, ICreateInvoiceNoteItemControls} from './models/create-invoice-note-form.model';
import {catchError, map, Observable, of, shareReplay, take, tap} from 'rxjs';
import {Button} from 'primeng/button';
import {Dialog} from 'primeng/dialog';
import {NumberFieldComponent} from '../../../../../shared/components/number-field/number-field.component';
import {CommonModule} from '@angular/common';
import {ISelectItem} from '../../../../../shared/components/single-select/models/single-select.model';
import {SingleSelectComponent} from '../../../../../shared/components/single-select/single-select.component';

@Component({
  selector: 'app-create-invoice-note-dialog',
  standalone: true,
  imports: [
    Button,
    Dialog,
    FormsModule,
    NumberFieldComponent,
    ReactiveFormsModule,
    CommonModule,
    SingleSelectComponent,
  ],
  templateUrl: './create-invoice-note-dialog.component.html',
  styleUrl: './create-invoice-note-dialog.component.css',
})
export class CreateInvoiceNoteDialogComponent implements OnInit {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();

  invoiceNoteForm!: FormGroup<ICreateInvoiceNoteControls>;
  products$!: Observable<IApiResult<IProduct[]>>;
  availableSelectItems$!: Observable<ISelectItem[]>;
  isSaving = false;

  private productList: IProduct[] = [];
  selectedProductId: string | null = null;
  tempQuantity = 1;

  constructor(
    private fb: NonNullableFormBuilder,
    private stockService: StockService,
    private billingService: BillingService,
    private toastService: ToastService,
  ) {
  }

  ngOnInit(): void {
    this.products$ = this.fetchProducts().pipe(
      shareReplay(1),
      tap(result => this.productList = result.data || [])
    );

    this.availableSelectItems$ = this.products$.pipe(
      map(result => result.data?.map(product => ({
        label: `${product.description} - ${product.description} (Estoque: ${product.stockBalance})`,
        value: product.id
      })) || [])
    );

    this.invoiceNoteForm = this.fb.group({
      items: this.fb.array<FormGroup<ICreateInvoiceNoteItemControls>>([], [Validators.required, Validators.minLength(1)])
    });
  }

  get itemsFormArray(): FormArray<FormGroup<ICreateInvoiceNoteItemControls>> {
    return this.invoiceNoteForm.controls.items;
  }

  private fetchProducts(): Observable<IApiResult<IProduct[]>> {
    return this.stockService.getAllProducts().pipe(
      catchError(error => {
        console.error(error);
        this.toastService.showError('Falha ao buscar produtos', 'Não foi possível carregar os produtos.');
        return of({isSuccess: false, messages: [], data: []});
      })
    );
  }

  createItemFormGroup(productId: string, quantity: number) {
    return this.fb.group<ICreateInvoiceNoteItemControls>({
      productId: this.fb.control(productId, Validators.required),
      quantity: this.fb.control(quantity, [Validators.required, Validators.min(1)])
    });
  }

  getProductName(productId: string | null): string {
    if (!productId) return 'Produto Desconhecido';

    const product = this.productList.find(p => p.id === productId);

    return product ? product.description : 'Produto Desconhecido';
  }

  addItem(): void {
    if (!this.selectedProductId) {
      this.toastService.showWarning('Atenção', 'Selecione um produto antes de adicionar.');
      return;
    }
    if (this.tempQuantity < 1) {
      this.toastService.showWarning('Atenção', 'Quantidade mínima é 1.');
      return;
    }

    const exists = this.itemsFormArray
      .controls
      .some(ctrl =>
        ctrl.controls.productId.value === this.selectedProductId);

    if (exists) {
      this.toastService.showWarning('Atenção', 'Produto já adicionado. Edite a quantidade na lista.');
      return;
    }

    this.itemsFormArray.push(this.createItemFormGroup(this.selectedProductId, this.tempQuantity));

    this.selectedProductId = null;
    this.tempQuantity = 1;
  }

  removeItem(index: number): void {
    this.itemsFormArray.removeAt(index);
  }

  resetForm(): void {
    this.invoiceNoteForm.reset();
    this.itemsFormArray.clear();
    this.selectedProductId = null;
    this.tempQuantity = 1;
    this.isSaving = false;
  }

  closeDialog(): void {
    this.resetForm();
    this.close.emit();
  }

  saveInvoiceNote(): void {
    this.invoiceNoteForm.markAllAsTouched();

    if (this.invoiceNoteForm.invalid) {
      this.toastService.showError('Falha', 'Preencha todos os campos obrigatórios.');
      return;
    }

    this.isSaving = true;
    const newInvoiceNote = this.invoiceNoteForm.getRawValue();

    this.billingService.createInvoiceNote(newInvoiceNote).subscribe({
      next: result => {
        if (result.isSuccess) {
          result.messages?.forEach(msg => this.toastService.showSuccess('Sucesso', msg));
          this.resetForm();
          this.save.emit();
        }
      },
      error: err => {
        console.error(err);
        const errorBody = err.error as IApiResult<any>;
        this.toastService.showApiError(errorBody, 'Falha ao salvar nota fiscal!')
        this.isSaving = false;
      }
    });
  }
}
