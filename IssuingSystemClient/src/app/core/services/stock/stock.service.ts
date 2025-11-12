import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {IApiResult} from '../../../shared/models/default.model';
import {ICreateProduct, IProduct} from '../../../shared/models/product.model';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private readonly baseUrl = 'http://localhost:6006/api/stock';
  private readonly productUrl = `${this.baseUrl}/v1/products`;

  constructor(private http: HttpClient) {}


  getAllProducts(): Observable<IApiResult<IProduct[]>> {
    return this.http.get<IApiResult<IProduct[]>>(this.productUrl);
  }

  createProduct(data: ICreateProduct): Observable<IApiResult<IProduct>> {
    return this.http.post<IApiResult<IProduct>>(this.productUrl, data);
  }
}
