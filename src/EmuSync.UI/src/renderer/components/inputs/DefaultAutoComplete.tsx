import { Autocomplete, AutocompleteProps, TextField } from "@mui/material";
import { ControllerFieldState, ControllerRenderProps } from "react-hook-form";

type DefaultAutoCompleteProps<
    T,
    Multiple extends boolean | undefined = false,
    DisableClearable extends boolean | undefined = false,
    FreeSolo extends boolean | undefined = false
> = Omit<
    AutocompleteProps<T, Multiple, DisableClearable, FreeSolo>,
    "renderInput" | "value" | "onChange"
> & {
    field: ControllerRenderProps<any, any>;
    fieldState?: ControllerFieldState;
    label?: string;
    placeholder?: string;
};

export default function DefaultAutoComplete<
    T,
    Multiple extends boolean | undefined = false,
    DisableClearable extends boolean | undefined = false,
    FreeSolo extends boolean | undefined = false
>({
    field,
    fieldState,
    label,
    placeholder,
    ...props
}: DefaultAutoCompleteProps<T, Multiple, DisableClearable, FreeSolo>) {
    return <Autocomplete
        {...props}
        value={field.value ?? null}
        onChange={(_, value) => field.onChange(value)}
        renderInput={(params) => (
            <TextField
                {...params}
                label={label}
                error={!!fieldState?.error}
                helperText={fieldState?.error?.message}
                variant="outlined"
                fullWidth
                placeholder={placeholder}
                slotProps={{
                    inputLabel: {
                        shrink: true,
                    }
                }}
            />
        )}
    />
}