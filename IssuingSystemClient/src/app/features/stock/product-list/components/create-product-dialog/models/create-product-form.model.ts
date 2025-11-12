import {ICreateProduct} from '../../../../../../shared/models/product.model';
import {FormControl} from '@angular/forms';


export interface ICreateProductControls {
  code: FormControl<ICreateProduct['code']>;
  description: FormControl<ICreateProduct['description']>;
  initialStockBalance: FormControl<ICreateProduct['initialStockBalance']>;
}

