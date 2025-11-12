import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { merge, timer, Subject, Observable, of, switchMap, catchError, finalize, takeUntil } from 'rxjs';
import { TableComponent } from '../../../shared/components/table/table.component';
import { IColumn } from '../../../shared/components/table/models/table.model';
import { IProduct } from '../../../shared/models/product.model';
import { IApiResult } from '../../../shared/models/default.model';
import { StockService } from '../../../core/services/stock/stock.service';
import { ToastService } from '../../../core/services/snackbar/toast.service';
import { CreateProductDialogComponent } from './components/create-product-dialog/create-product-dialog.component';
import { Button } from 'primeng/button';
import { formatDate } from '../../../shared/utils/date.utils';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, TableComponent, Button, CreateProductDialogComponent],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
})
export class ProductListComponent implements OnInit, OnDestroy {
  private readonly POLLING_INTERVAL = 30000;
  private readonly destroy$ = new Subject<void>();
  private readonly refreshTrigger$ = new Subject<void>();

  products$!: Observable<IApiResult<IProduct[]>>;
  loading = false;
  showCreateDialog = false;

  readonly COLUMNS: IColumn[] = [
    { header: 'Código', field: 'code' },
    { header: 'Descrição', field: 'description' },
    { header: 'Quantidade', field: 'stockBalance' },
    { header: 'Criado Em', field: 'createdAt', render: formatDate },
  ];

  constructor(private stockService: StockService, private toastService: ToastService) {}

  ngOnInit(): void {
    const polling$ = timer(0, this.POLLING_INTERVAL);

    this.products$ = merge(polling$, this.refreshTrigger$).pipe(
      switchMap(() => this.loadProducts()),
      takeUntil(this.destroy$)
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProducts(): Observable<IApiResult<IProduct[]>> {
    this.loading = true;

    return this.stockService.getAllProducts().pipe(
      finalize(() => (this.loading = false)),
      catchError((err: HttpErrorResponse) => {
        console.error('Falha ao buscar produtos:', err);

        this.toastService.showApiError(err.error, 'Falha ao buscar produtos.');

        return of({ isSuccess: false, messages: [], data: [] });
      })
    );
  }

  refreshProducts(): void {
    this.refreshTrigger$.next();
  }

  openCreateProductDialog(): void {
    this.showCreateDialog = true;
  }

  closeCreateProductDialog(saved: boolean = false): void {
    this.showCreateDialog = false;
    if (saved) {
      this.refreshProducts();
    }
  }
}
