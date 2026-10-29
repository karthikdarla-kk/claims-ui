import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export type ClaimStatus = 'Submitted' | 'InReview' | 'AwaitingInformation' | 'Settled' | 'Rejected';
export interface Claim { id: string; claimNumber: string; claimantName: string; claimantEmail: string; type: 'Motor' | 'Property'; incidentDate: string; description: string; estimatedLoss: number; status: ClaimStatus; assignedOfficer?: string; decisionReason?: string; }
export interface Dashboard { totalClaims: number; openClaims: number; outstandingExposure: number; claimsByStatus: Record<string, number>; }

@Injectable({ providedIn: 'root' })
export class ClaimService {
  private readonly api = 'https://localhost:7185/api';
  constructor(private readonly http: HttpClient) {}
  getClaims(email?: string): Observable<Claim[]> { return this.http.get<Claim[]>(`${this.api}/claims`, { params: email ? { email } : {} }); }
  getDashboard(): Observable<Dashboard> { return this.http.get<Dashboard>(`${this.api}/dashboard`); }
  createClaim(request: Omit<Claim, 'id' | 'claimNumber' | 'status'>): Observable<Claim> { return this.http.post<Claim>(`${this.api}/claims`, request); }
  updateClaim(id: string, request: { status: ClaimStatus; decisionReason?: string; assignedOfficer?: string }): Observable<Claim> { return this.http.patch<Claim>(`${this.api}/claims/${id}`, request); }
}
