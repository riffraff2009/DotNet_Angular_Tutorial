import { Photo } from "./photo"

export interface Member {
    id: number
    userName: string
    photoUrl: string
    age: number
    gender: string
    knownAs: string
    created: string
    lastActive: string
    aboutMe: string
    lookingFor: string
    interests: string
    city: string
    country: string
    photos: Photo[]
  }
  
