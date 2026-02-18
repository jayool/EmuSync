
import DisplayDate from "@/renderer/components/dates/DisplayDate";
import ShowModal from "@/renderer/components/modals/ShowModal";
import HorizontalStack from "@/renderer/components/stacks/HorizontalStack";
import { GameBackupManifest } from "@/renderer/types";
import { Button, Divider, IconButton, List, ListItem, ListItemText, Paper, Typography } from "@mui/material";
import React, { useEffect, useMemo, useState } from "react";
import DeleteIcon from '@mui/icons-material/Delete';
import CloseIcon from '@mui/icons-material/Close';

interface RestoreFromBackupModalProps {
    backups: GameBackupManifest[];
    isOpen: boolean;
    setIsOpen: (open: boolean) => void;
    onSelectBackup: (backupId: string) => void;
    onDeleteBackup: (backupId: string) => Promise<void>;
}

export default function RestoreFromBackupModal({
    backups,
    isOpen, setIsOpen,
    onSelectBackup, onDeleteBackup
}: RestoreFromBackupModalProps) {

    useEffect(() => {
        if (backups.length === 0) {
            setIsOpen(false);
        }
    }, [backups]);

    const orderedBackups = useMemo(() => {
        return backups.sort((a, b) => {
            const dateA = new Date(a.createdOnUtc).getTime();
            const dateB = new Date(b.createdOnUtc).getTime();

            return dateB - dateA;
        });
    }, [backups]);

    const BackupsListMemo = useMemo(() => {

        return orderedBackups.map((backup, index) => {
            return <React.Fragment
                key={backup.id}
            >
                <BackupListItem
                    backup={backup}
                    onSelect={onSelectBackup}
                    onDelete={onDeleteBackup}
                />

                {
                    index < (orderedBackups.length - 1) &&
                    <Divider component="li" />
                }


            </React.Fragment>

        })

    }, [orderedBackups, onSelectBackup]);

    return <ShowModal
        isOpen={isOpen}
        setIsOpen={setIsOpen}
        title="Select a backup"
        showCloseButton
        maxWidth="md"
    >

        <List
            component={Paper}
            elevation={3}
        >
            {BackupsListMemo}
        </List>
    </ShowModal>

}

interface BackupListItemProps {
    backup: GameBackupManifest;
    onSelect: (id: string) => void;
    onDelete: (id: string) => Promise<void>;
}

function BackupListItem({
    backup,
    onSelect, onDelete
}: BackupListItemProps) {

    const [confirmDelete, setConfirmDelete] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);

    const date = new Date(backup.createdOnUtc);

    return <ListItem
        sx={{
            my: 1
        }}
        secondaryAction={
            confirmDelete ?
                <HorizontalStack>
                    <Button
                        variant="contained"
                        color="error"
                        size="small"
                        onClick={async () => {
                            setIsDeleting(true);
                            await onDelete(backup.id)
                        }}
                        title="Delete this backup"
                        loading={isDeleting}
                    >
                        Delete this backup
                    </Button>
                    <IconButton
                        title="Cancel delete"
                        onClick={() => setConfirmDelete(false)}
                    >
                        <CloseIcon
                            color="secondary"
                        />
                    </IconButton>
                </HorizontalStack>
                :
                <HorizontalStack>
                    <Button
                        variant="contained"
                        color="primary"
                        size="small"
                        onClick={() => onSelect(backup.id)}
                    >
                        Use this backup
                    </Button>
                    <IconButton
                        title="Delete backup"
                        onClick={() => setConfirmDelete(true)}
                    >
                        <DeleteIcon
                            color="error"
                        />
                    </IconButton>
                </HorizontalStack>

        }
    >
        <ListItemText
            primary={
                <HorizontalStack gap={0.5}>
                    <Typography>
                        A backup taken
                    </Typography>
                    <DisplayDate
                        date={date}
                        displayAsFromNow
                    />
                </HorizontalStack>
            }
        />
    </ListItem>
}