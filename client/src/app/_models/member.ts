import {Photo} from "./photo";

export interface Member {
  id: number
  username: string
  age: number
  knownAs: string
  photoUrl: string
  created: string
  lastActive: string
  gender: string
  introduction: string
  lookingFor: string
  interests: string
  city: string
  country: string
  photos: Photo[]
}

