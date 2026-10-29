import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Claim, ClaimService, ClaimStatus, Dashboard } from './claim.service';

@Component({ selector: 'app-root', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './app.component.html' })
export class AppComponent implements OnInit {
  role: 'claimant' | 'officer' = 'claimant';
  claims: Claim[] = [];
  dashboard?: Dashboard;
  message = '';
  claimantEmail = 'aisha@example.com';
  newClaim = { claimantName: '', claimantEmail: '', type: 'Motor' as 'Motor' | 'Property', incidentDate: '', description: '', estimatedLoss: 0 };
  constructor(private readonly claimsApi: ClaimService) {}
  ngOnInit(): void { this.load(); }
  load(): void { if (this.role === 'claimant') this.claimsApi.getClaims(this.claimantEmail).subscribe({ next: claims => this.claims = claims, error: () => this.message = 'Start the API first, then refresh.' }); else { this.claimsApi.getClaims().subscribe(claims => this.claims = claims); this.claimsApi.getDashboard().subscribe(data => this.dashboard = data); } }
  switchRole(): void { this.role = this.role === 'claimant' ? 'officer' : 'claimant'; this.message = ''; this.load(); }
  submit(): void { this.claimsApi.createClaim(this.newClaim).subscribe({ next: claim => { this.claims = [claim, ...this.claims]; this.message = `${claim.claimNumber} submitted successfully.`; this.newClaim = { claimantName: '', claimantEmail: '', type: 'Motor', incidentDate: '', description: '', estimatedLoss: 0 }; }, error: error => this.message = error.error?.message ?? 'Could not submit claim.' }); }
  progress(claim: Claim, status: ClaimStatus): void { const reason = status === 'Settled' || status === 'Rejected' ? `${status} following assessment.` : undefined; this.claimsApi.updateClaim(claim.id, { status, assignedOfficer: status === 'InReview' ? 'Demo Officer' : claim.assignedOfficer, decisionReason: reason }).subscribe({ next: updated => { claim.status = updated.status; claim.assignedOfficer = updated.assignedOfficer; claim.decisionReason = updated.decisionReason; this.load(); } }); }
  formatStatus(status: string): string { return status.replace(/([a-z])([A-Z])/g, '$1 $2'); }
}
