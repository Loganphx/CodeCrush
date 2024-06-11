import {Component, OnInit} from '@angular/core';
import {Photo, PhotoForApproval} from "../../_models/photo";
import {AdminService} from "../../_services/admin.service";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.scss']
})
export class PhotoManagementComponent implements OnInit {
  photos: PhotoForApproval[] = [];
  constructor(private adminService: AdminService,
              private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.getUnapprovedPhotos();
  }

  getUnapprovedPhotos(): void {
    this.adminService.getUnapprovedPhotos().subscribe({
      next: photos => {
        this.photos = photos;
        // this.pagination = users.pagination;
      },
      error: err => this.toastr.error(err)
    })
  }

  approvePhoto(photoId: number): void {
    this.adminService.approvePhoto(photoId).subscribe({
      next: _ => this.photos = [...this.photos.filter(p => p.photoId !== photoId)],
      error: err => this.toastr.error(err)
    });
  }
  rejectPhoto(photoId: number): void {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: _ => this.photos = [...this.photos.filter(p => p.photoId !== photoId)],
      error: err => this.toastr.error(err)
    });
  }
}
