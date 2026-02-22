import SuccessAlert from "@/renderer/components/alerts/SuccessAlert";
import WarningAlert from "@/renderer/components/alerts/WarningAlert";
import { Game } from "@/renderer/types";
import { SxProps } from "@mui/material";

interface QuickAddGameStatusProps {
    existingGame: Game | null;
}

const sx: SxProps = {
    m: 0,
    py: 0,
    px: 1
}

export default function QuickAddGameStatus({
    existingGame
}: QuickAddGameStatusProps) {

    if (!existingGame) {
        return <SuccessAlert
            title="The save file location is the same as the existing game you've selected"
            content="A new game will be created"
            sx={sx}
        />
    }

    return <WarningAlert
        title="An existing game has been found or selected"
        content="An existing game will be updated"
        sx={sx}
    />
}