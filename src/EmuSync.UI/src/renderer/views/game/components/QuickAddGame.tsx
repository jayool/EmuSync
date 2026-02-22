import PickDirectoryButton from "@/renderer/components/buttons/PickDirectoryButton";
import DefaultAutoComplete from "@/renderer/components/inputs/DefaultAutoComplete";
import DefaultCheckbox from "@/renderer/components/inputs/DefaultCheckbox";
import DefaultTextField from "@/renderer/components/inputs/DefaultTextField";
import HorizontalStack from "@/renderer/components/stacks/HorizontalStack";
import VerticalStack from "@/renderer/components/stacks/VerticalStack";
import { GameSummary } from "@/renderer/types";
import QuickAddGameStatus from "@/renderer/views/game/components/QuickAddGameStatus";
import { QuickAddGamesForm } from "@/renderer/views/game/utils/quick-add-utils";
import DeleteIcon from '@mui/icons-material/Delete';
import { Box, Divider, IconButton, Paper } from "@mui/material";
import { Control, Controller, useWatch } from "react-hook-form";

interface QuickAddGameProps {
    index: number;
    fullGameList: GameSummary[];
    control: Control<QuickAddGamesForm>;
    disabled?: boolean;
    onRemoveGame: (gameIndex: number) => void;
}

export default function QuickAddGame({
    index, fullGameList, control, disabled,
    onRemoveGame
}: QuickAddGameProps) {

    const isNewGameOnly = useWatch({ name: `games.${index}.isNewGameOnly`, control });
    const existingGame = useWatch({ name: `games.${index}.existingGame`, control });

    return <Paper
        elevation={2}
        sx={{
            p: 2
        }}
        component={VerticalStack}
    >
        <HorizontalStack>

            <QuickAddGameStatus
                existingGame={existingGame}
            />

            <IconButton
                title="Remove game"
                onClick={() => onRemoveGame(index)}
                sx={{
                    ml: "auto"
                }}
                disabled={disabled}
            >
                <DeleteIcon
                    color="error"
                />
            </IconButton>

        </HorizontalStack>

        <Divider />

        {
            !isNewGameOnly &&
            <Controller
                name={`games.${index}.existingGame` as const}
                control={control as never}

                render={({ field, fieldState }) => {

                    return <DefaultAutoComplete
                        field={field}
                        fieldState={fieldState}
                        label="Existing game"
                        disabled={disabled}
                        options={fullGameList}

                        getOptionLabel={(option) => option.name}
                        placeholder="Select an existing game to update it"
                        size="small"
                    />
                }}
            />
        }


        {
            !existingGame &&
            <Controller
                name={`games.${index}.name` as const}
                control={control as never}
                rules={{
                    required: "Name is required",
                    maxLength: { value: 255, message: "Name must be 255 characters or less" }
                }}
                render={({ field, fieldState }) => {

                    return <DefaultTextField
                        required
                        field={field}
                        fieldState={fieldState}
                        label="Game name"
                        disabled={disabled}
                        placeholder="Enter a name for the game"
                        size="small"
                    />
                }}
            />
        }

        <Controller
            name={`games.${index}.maxLocalBackups` as const}
            control={control}
            rules={{
                min: { value: 0, message: "Must be greater than -1" },
                validate: (v) => Number.isInteger(Number(v)) || "Must be a whole number"
            }}
            render={({ field, fieldState }) => (
                <DefaultTextField
                    field={field}
                    fieldState={fieldState}
                    label="Maximum local game backups"
                    type="number"
                    disabled={disabled}
                    placeholder="Override the maximum amount of local backups kept for this game"
                    size="small"
                />
            )}
        />

        <Controller
            name={`games.${index}.path` as const}
            control={control as never}
            rules={{
                required: "Sync location is required"
            }}
            render={({ field, fieldState }) => {

                return <HorizontalStack>
                    <DefaultTextField
                        required
                        field={field}
                        fieldState={fieldState}
                        placeholder="Pick or enter a location for this device"
                        label="Sync location"
                        disabled={disabled}
                        size="small"
                    />

                    <PickDirectoryButton
                        disabled={disabled}
                        defaultPath={field.value}
                        onPickDirectory={(directory) => field.onChange(directory)}
                    />
                </HorizontalStack>
            }}
        />


        <Box
            sx={{
                px: 1
            }}
        >

            <Controller
                name={`games.${index}.autoSync` as const}
                control={control as never}
                render={({ field }) => (
                    <DefaultCheckbox
                        field={field}
                        checked={field.value || false}
                        onChange={(e) => field.onChange(e.target.checked)}
                        disabled={disabled}
                        label="Automatically sync this game?"
                    />
                )}
            />
        </Box>

    </Paper>
}