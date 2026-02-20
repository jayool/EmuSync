import { Game, QuickAddRequestBody, SyncSource } from "@/renderer/types";


export interface QuickAddGameClientModel {
    name: string;
    path: string;
    maxLocalBackups: number | null;
    existingGame: Game | null;
    autoSync: boolean;
    isNewGameOnly: boolean;
}


export interface QuickAddGamesForm {
    games: QuickAddGameClientModel[];
}

export function getDefaultValues(): QuickAddGamesForm {
    return {
        games: []
    }
}

export function convertToRequestBody(form: QuickAddGamesForm): QuickAddRequestBody {

    const output: QuickAddRequestBody = {
        games: form.games.map(game => {

            const gameExists = game.existingGame !== null;

            return {
                existingGameId: game.existingGame?.id ?? null,
                path: game.path,
                gameName: gameExists ? null : game.name,
                autoSync: game.autoSync,
                maximumLocalGameBackups: game.maxLocalBackups?.toString() === "" ? null : game.maxLocalBackups
            }
        })
    };

    return output;
}

export function filePathIsUnchanged(existingGame: Game | null, gamePath: string, syncSource: SyncSource) {
    return existingGame?.syncSourceIdLocations?.[syncSource.id] === gamePath;
}