import WarningAlert from "@/renderer/components/alerts/WarningAlert";
import HorizontalStack from "@/renderer/components/stacks/HorizontalStack";
import { Box, Typography } from "@mui/material";

interface SectionTitleProps {
    title: string;
    icon?: React.ReactNode;
    sectionIsDirty?: boolean;
    endAdornment?: React.ReactNode;
}

export default function SectionTitle({
    title, icon, sectionIsDirty,
    endAdornment
}: SectionTitleProps) {
    return <HorizontalStack sx={{ height: 40 }}>
        {
            typeof icon !== "undefined" &&
            <>
                {icon}
            </>
        }
        <Typography variant="h6">
            {title}
        </Typography>
        {
            sectionIsDirty ?
                <WarningAlert
                    sx={{
                        ml: "auto",
                        p: 0,
                        px: 1,
                    }}
                    content="You have unsaved changes"
                />
                :
                <Box

                    sx={{
                        ml: "auto",
                    }}
                >
                    {endAdornment}
                </Box>
        }
    </HorizontalStack>
}