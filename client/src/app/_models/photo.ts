export interface Photo {
  id: number
  url: string
  isMain: boolean
  isApproved: boolean
}

export interface PhotoForApproval {
  photoId: number
  url: string
  username: string
  isApproved: boolean
}
