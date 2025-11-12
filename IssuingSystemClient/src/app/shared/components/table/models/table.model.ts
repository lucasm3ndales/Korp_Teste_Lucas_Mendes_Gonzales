

export interface IColumn {
  header: string;
  field: string;
  width?: number;
  render?: (value: any) => string;
}
