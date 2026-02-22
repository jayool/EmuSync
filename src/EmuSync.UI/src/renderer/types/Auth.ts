
export interface DropboxAuthUrlResponse {
    url: string;
    state: string;
}

export interface GoogleAuthUrlResponse {
    url: string;
}

export interface MicrosoftAuthUrlResponse {
    url: string;
}

export interface SharedFolderAuthFinish {
    path: string;
    username: string | null;
    password: string | null;
}