import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

import { Subject } from 'rxjs/internal/Subject';
import { debounceTime } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private queryModelSubject: Subject<string> = new Subject<string>();
  private queryResponseSubject: Subject<any> = new Subject<any>();

  constructor(private http: HttpClient) { 
    this.subscribeOnQueryModel();
  } 

  public queryValueSubjectNextValue(query: string) {
    this.queryModelSubject.next(query);
  }

  private onQueryChanged(query: string) {
    this.search(query).subscribe(result => {
      console.log('query result');
      console.log(result);
    });
  }

  public subscribeOnQueryModel() {
    this.queryModelSubject
      .pipe(debounceTime(300))
      .subscribe(model => {
        this.onQueryChanged(model);
      });
  }

  public subscribeOnQueryResponse(next?: (value: any) => void, error?: (error: any) => void, complete?: () => void) {
    this.queryResponseSubject.subscribe(next, error, complete);
  }

  private search(query: string): any {
    return this.http.get(`${environment.server.url}/api/search?query=${query}`); 
  }
}
