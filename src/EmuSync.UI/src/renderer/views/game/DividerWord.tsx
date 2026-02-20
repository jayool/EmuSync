import { Box, Typography } from "@mui/material";

interface DividerWordProps {
    word: string;
}

export default function DividerWord({
    word
}: DividerWordProps) {
    return (
        <Box
            display="flex"
            alignItems="center"
            width="100%"
        >
            <Box
                flex="1"
                height={2}
                bgcolor="divider"
            />
            <Typography
                sx={{ mx: 1 }}
                color="text.secondary"
                variant="body2"
            >
                {word}
            </Typography>
            <Box
                flex="1"
                height={2}
                bgcolor="divider"
            />
        </Box>
    );
}