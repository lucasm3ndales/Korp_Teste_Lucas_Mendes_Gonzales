import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {CommonModule} from '@angular/common';
import {Dialog} from 'primeng/dialog';
import {Button} from 'primeng/button';
import {FormGroup, NonNullableFormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {TextFieldComponent} from '../../../../../shared/components/text-field/text-field.component';
import {NumberFieldComponent} from '../../../../../shared/components/number-field/number-field.component';
import {ICreateProduct} from '../../../../../shared/models/product.model';
import {StockService} from '../../../../../core/services/stock/stock.service';
import {ToastService} from '../../../../../core/services/snackbar/toast.service';
import {IApiResult} from '../../../../../shared/models/default.model';
import {HttpErrorResponse} from '@angular/common/http';
import {finalize} from 'rxjs';
import {ICreateProductControls} from './models/create-product-form.model';

@Component({
  selector: 'app-create-product-dialog',
  standalone: true,
  imports: [
    CommonModule,
    Dialog,
    Button,
    ReactiveFormsModule,
    TextFieldComponent,
    NumberFieldComponent,
  ],
  templateUrl: './create-product-dialog.component.html',
  styleUrl: './create-product-dialog.component.css',
})
export class CreateProductDialogComponent implements OnInit {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<void>();
  isSaving = false;

  productForm!: FormGroup<ICreateProductControls>;

  constructor(
    private fb: NonNullableFormBuilder,
    private stockService: StockService,
    private toastService: ToastService,
  ) {
  }

  ngOnInit(): void {
    this.buildForm();
  }

  private buildForm(): void {
    this.productForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(1)]],
      initialStockBalance: [0, [Validators.required, Validators.min(0)]],
    });
  }

  closeDialog(): void {
    this.productForm.reset();
    this.close.emit();
  }

  saveProduct(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const newProduct: ICreateProduct = this.productForm.getRawValue();

    this.stockService.createProduct(newProduct)
      .pipe(finalize(() => (this.isSaving = false)))
      .subscribe({
        next: (result) => {
          if (result.isSuccess) {
            this.toastService.showApiSuccess(result, "Produto salvo com sucesso!");
            this.closeDialog();
            this.save.emit();
          }
        },
        error: (err: HttpErrorResponse) => {
          const errorBody = err.error as IApiResult<any>;
          this.toastService.showApiError(errorBody, 'Falha ao salvar produto!');
        },
      });
  }
}
