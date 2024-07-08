import http from 'k6/http';
import { sleep } from 'k6';
export const options = {
    vus: 500,
    duration: '30s',
  };
export default function () {
  http.get('https://localhost:7247/api/Invoices?page=1&pageSize=10');
  sleep(1);
}