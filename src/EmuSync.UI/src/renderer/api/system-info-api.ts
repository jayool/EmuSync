import { get, postWithNoResponse } from "@/renderer/api/api-helper";
import { SystemInfo } from "@/renderer/types";

const controller = "System"

export async function checkApiIsRunning(): Promise<boolean> {

    const path = `${controller}/HealthCheck`;

    await postWithNoResponse({
        path
    });

    return true;
}