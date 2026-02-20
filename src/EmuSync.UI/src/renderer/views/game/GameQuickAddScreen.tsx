import { cacheKeys } from "@/renderer/api/cache-keys";
import { clearGameCache, getGameList, quickAddGames } from "@/renderer/api/game-api";
import BackToListButton from "@/renderer/components/buttons/BackToListButton";
import ButtonRow from "@/renderer/components/buttons/ButtonRow";
import GameSuggestionAutocomplete from "@/renderer/components/inputs/GameSuggestionAutocomplete";
import { Pre } from "@/renderer/components/Pre";
import Section from "@/renderer/components/Section";
import SectionTitle from "@/renderer/components/SectionTitle";
import VerticalStack from "@/renderer/components/stacks/VerticalStack";
import useAlerts from "@/renderer/hooks/use-alerts";
import useListQuery from "@/renderer/hooks/use-list-query";
import { routes } from "@/renderer/routes";
import { localSyncSourceAtom } from "@/renderer/state/local-sync-source";
import { Game, GameSuggestion, GameSummary, QuickAddRequestBody } from "@/renderer/types";
import QuickAddGame from "@/renderer/views/game/components/QuickAddGame";
import DividerWord from "@/renderer/views/game/DividerWord";
import { QuickAddGameClientModel, QuickAddGamesForm, convertToRequestBody, getDefaultValues } from "@/renderer/views/game/utils/quick-add-utils";
import { KeyboardReturnOutlined } from "@mui/icons-material";
import { Autocomplete, Box, Button, Divider, Grid, Paper, TextField, Typography } from "@mui/material";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { error } from "console";
import { useAtom } from "jotai";
import { useCallback, useMemo, useState } from "react";
import { useFieldArray, useForm, useWatch } from "react-hook-form";

const Icon = routes.gameQuickAdd.icon;

export default function GameQuickAddScreen() {

    const [localSyncSource] = useAtom(localSyncSourceAtom);
    const { successAlert, errorAlert } = useAlerts();
    const queryClient = useQueryClient();
    const [existingGameInput, setExistingGameInput] = useState("");

    const {
        query
    } = useListQuery({
        queryFn: async () => getGameList(),
        resetCacheFn: clearGameCache,
        queryKey: [cacheKeys.gameList],
        relatedQueryKeys: [cacheKeys.gameList],
        mutationFn: async () => { }
    });

    const fullGameList = query.data ?? [];

    const {
        handleSubmit,
        control,
        reset,
        watch
    } = useForm<QuickAddGamesForm>({
        defaultValues: getDefaultValues()
    });

    const watchedGames = useWatch({
        control,
        name: "games"
    });

    const {
        fields: selectedGames,
        append,
        remove
    } = useFieldArray({
        control,
        name: "games"
    });

    const updateMutation = useMutation({
        mutationFn: (body: QuickAddRequestBody) => quickAddGames(body),
        onSuccess: async (data) => {

            queryClient.invalidateQueries({ queryKey: [cacheKeys.gameList] });

            reset();
            successAlert("Games added/updated successfully");
        },
        onError: async () => {
            errorAlert("An error occurred");
        },
    });

    const remainingGames = useMemo(() => {

        if (!watchedGames || watchedGames.length === 0) {
            return fullGameList;
        }

        const selectedIds = new Set(
            watchedGames.filter(game => !!game.existingGame).map(game => game.existingGame!.id)
        );

        return fullGameList.filter(game =>
            !selectedIds.has(game.id)
        );

    }, [watchedGames, fullGameList]);

    const handleFormSubmit = useCallback((data: QuickAddGamesForm) => {
        const body = convertToRequestBody(data);
        console.log(body);
        updateMutation.mutate(body)
    }, []);

    const handleGameSuggestionSelect = useCallback((game: GameSuggestion, filePath: string) => {

        const existingGame = remainingGames.find(g => g.name === game.name) ?? null;

        const quickAddGame: QuickAddGameClientModel = {
            name: game.name,
            path: filePath,
            maxLocalBackups: existingGame?.maximumLocalGameBackups ?? null,
            existingGame,
            autoSync: existingGame?.autoSync ?? false,
            isNewGameOnly: false
        }

        append(quickAddGame);

    }, [append, remainingGames]);

    const handleExistingGameSelect = useCallback((game: GameSummary | null) => {

        setExistingGameInput(""); // immediately clear the input

        if (game === null) return;

        const filePath = game.syncSourceIdLocations?.[localSyncSource.id] ?? "";

        const quickAddGame: QuickAddGameClientModel = {
            name: game.name,
            path: filePath,
            maxLocalBackups: game.maximumLocalGameBackups,
            existingGame: game,
            autoSync: game.autoSync,
            isNewGameOnly: false
        }

        append(quickAddGame);

    }, [append, localSyncSource]);

    const addEmptyGame = useCallback(() => {

        const quickAddGame: QuickAddGameClientModel = {
            name: "",
            path: "",
            maxLocalBackups: null,
            existingGame: null,
            autoSync: false,
            isNewGameOnly: true
        }

        append(quickAddGame);

    }, [append]);


    const handleGameSuggestionRemove = useCallback((gameIndex: number) => {
        remove(gameIndex);
    }, [remove]);

    const isSubmitting = updateMutation.isPending;

    return <Grid container gap={2}>

        <Grid
            size={{
                xs: 12,
                lg: 5
            }}
        >
            <Box
                sx={{
                    position: {
                        xs: "initial",
                        lg: "sticky"
                    },
                    top: {
                        xs: undefined,
                        lg: 0
                    },
                }}
            >
                <Section>
                    <BackToListButton
                        href={routes.game.href}
                        disableFloat
                        disableMargin
                    />

                    <Divider />

                    <SectionTitle
                        title={routes.gameQuickAdd.title}
                        icon={<Icon />}
                    />

                    <GameSuggestionAutocomplete
                        disabled={query.isLoading || isSubmitting}
                        onSelect={handleGameSuggestionSelect}
                    />

                    <DividerWord word="OR" />

                    <Autocomplete
                        disabled={isSubmitting}
                        options={remainingGames}

                        getOptionLabel={(option) => option.name}
                        onChange={(e, value) => handleExistingGameSelect(value)}

                        // control the selected value (always null since we just append)
                        value={null}

                        // control the input text
                        inputValue={existingGameInput}
                        onInputChange={(e, newInputValue) => setExistingGameInput(newInputValue)}
                        disableCloseOnSelect

                        renderInput={(params) =>
                            <TextField
                                {...params}
                                placeholder="Select an existing game to update it"
                                label="Existing games"
                                slotProps={{
                                    inputLabel: {
                                        shrink: true,
                                    },
                                }}
                            />
                        }

                    />

                    <Divider />

                    <ButtonRow >

                        <form onSubmit={handleSubmit(handleFormSubmit)}>

                            <Button
                                variant="contained"
                                type="submit"
                                disabled={query.isLoading || isSubmitting || selectedGames.length === 0}
                                loading={isSubmitting}
                            >
                                Save
                            </Button>
                        </form>


                        <Button
                            variant="contained"
                            onClick={addEmptyGame}
                            color="success"
                            sx={{
                                ml: "auto"
                            }}
                            disabled={query.isLoading || isSubmitting}
                        >
                            Add game
                        </Button>

                    </ButtonRow>

                </Section>
            </Box>

        </Grid>

        <Grid
            size="grow"
        >

            {
                selectedGames.length > 0 ?
                    <VerticalStack>

                        {
                            selectedGames.map((game, index) => {

                                return <QuickAddGame
                                    key={(game as any).id}
                                    index={index}
                                    fullGameList={remainingGames}
                                    control={control}
                                    onRemoveGame={handleGameSuggestionRemove}
                                    disabled={isSubmitting}
                                />

                            })
                        }
                    </VerticalStack>
                    :
                    <InfoPlaceholder />
            }

        </Grid>

    </Grid>
}

function InfoPlaceholder() {
    return <Paper
        elevation={2}
        sx={{
            p: 2
        }}
        component={VerticalStack}
    >
        <Typography>
            Select a game from the suggestions list or use the <Pre>Add game</Pre> button to add a new game. You can only set the save file location for this device using this section.
        </Typography>

        <Typography>
            Existing games can be updated and are matched by name when you pick a suggestion. This isn't foolproof because names are not unique in EmuSync, so use with caution and always check the correct game has been selected.
        </Typography>
    </Paper>
}