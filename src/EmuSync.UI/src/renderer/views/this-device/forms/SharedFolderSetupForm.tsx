
import { completeSharedFolderSetup } from "@/renderer/api/auth-api";
import ButtonRow from "@/renderer/components/buttons/ButtonRow";
import PickDirectoryButton from "@/renderer/components/buttons/PickDirectoryButton";
import DefaultTextField from "@/renderer/components/inputs/DefaultTextField";
import ShowModal from "@/renderer/components/modals/ShowModal";
import HorizontalStack from "@/renderer/components/stacks/HorizontalStack";
import VerticalStack from "@/renderer/components/stacks/VerticalStack";
import useAlerts from "@/renderer/hooks/use-alerts";
import { localSyncSourceAtom } from "@/renderer/state/local-sync-source";
import { SharedFolderAuthFinish } from "@/renderer/types";
import { OsPlatform } from "@/renderer/types/enums";
import { normalisePathDelims } from "@/renderer/utils/path-utils";
import { Button, Divider } from "@mui/material";
import { useAtom } from "jotai";
import { useCallback, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";

interface SharedFolderSetupFormProps {
    isOpen: boolean;
    setIsOpen: (isOpen: boolean) => void;
    onConnected: () => void;
}

function getDefaultValues(): SharedFolderAuthFinish {
    return {
        path: "",
        password: "",
        username: ""
    }
}

export default function SharedFolderSetupForm({
    isOpen, setIsOpen,
    onConnected
}: SharedFolderSetupFormProps) {

    const [localSyncSource] = useAtom(localSyncSourceAtom);

    const thisDeviceIsWindows = useMemo(() => {
        return localSyncSource.platformId === OsPlatform.Windows;
    }, [localSyncSource]);

    const { successAlert, errorAlert } = useAlerts();
    const [isSubmitting, setIsSubmitting] = useState(false);

    const {
        handleSubmit,
        control,
        reset,
        formState,
        watch
    } = useForm<SharedFolderAuthFinish>({
        defaultValues: getDefaultValues()
    });

    const userName = watch("username");
    const password = watch("password");

    const handleCancel = useCallback(() => {
        setIsOpen(false);
        reset(
            getDefaultValues()
        );
    }, []);

    const handleFormSubmit = useCallback(async (data: SharedFolderAuthFinish) => {

        setIsSubmitting(true);

        try {

            data.path = normalisePathDelims(data.path, thisDeviceIsWindows);

            await completeSharedFolderSetup(data);
            onConnected();

            successAlert("Successfully connected to shared/local folder");

        } catch (ex) {
            console.error(ex);
            errorAlert("An error occurred saving the shared folder details. If you're using a network folder, double check the details you've entered.")
        } finally {
            setIsSubmitting(false);
        }


    }, [onConnected, thisDeviceIsWindows]);

    return <ShowModal
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        title="Set up a local/shared folder"
    >
        <form onSubmit={handleSubmit(handleFormSubmit)}>
            <VerticalStack>
                <Controller
                    name="path"
                    control={control as never}
                    rules={{ required: "Path is required" }}
                    render={({ field, fieldState }) => (
                        <HorizontalStack>
                            <DefaultTextField
                                required
                                field={field}
                                fieldState={fieldState}
                                label="Path"
                                disabled={isSubmitting}
                                placeholder={
                                    thisDeviceIsWindows
                                        ? `E.g., \\\\Your-Device\\SharedFolder or C:\\YourFolder`
                                        : "E.g., /home/deck/your-folder"
                                }
                            />

                            <PickDirectoryButton
                                disabled={isSubmitting}
                                defaultPath={field.value}
                                onPickDirectory={(directory) => field.onChange(directory)}
                            />
                        </HorizontalStack>

                    )}
                />

                {
                    thisDeviceIsWindows && <>
                        <Divider />

                        <Controller
                            name="username"
                            control={control as never}
                            rules={{
                                required: !!password && "Required when a password has been set"
                            }}
                            render={({ field, fieldState }) => (
                                <DefaultTextField
                                    required={!!userName || !!password}
                                    field={field}
                                    fieldState={fieldState}
                                    label="Username"
                                    disabled={isSubmitting}
                                    placeholder="Optional username if your folder requires credentials"
                                />
                            )}
                        />

                        <Controller
                            name="password"
                            control={control as never}
                            rules={{
                                required: !!userName && "Required when a username has been set"
                            }}
                            render={({ field, fieldState }) => (
                                <DefaultTextField

                                    required={!!userName || !!password}
                                    field={field}
                                    fieldState={fieldState}
                                    label="Password"
                                    disabled={isSubmitting}
                                    placeholder="Optional password if your folder requires credentials"
                                    type="password"
                                />
                            )}
                        />
                    </>
                }

                <Divider />
                <ButtonRow>
                    <Button
                        color="primary"
                        variant="contained"
                        disabled={isSubmitting || !formState.isDirty}
                        loading={isSubmitting}
                        type="submit"
                    >
                        Save
                    </Button>
                    <Button
                        color="secondary"
                        onClick={handleCancel}
                        variant="outlined"
                    >
                        Cancel
                    </Button>
                </ButtonRow>
            </VerticalStack>
        </form>

    </ShowModal>
}