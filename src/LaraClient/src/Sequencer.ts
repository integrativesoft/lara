/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

type resolver = (value?: boolean | PromiseLike<boolean>) => void;

export class Sequencer {

    private next: number;
    private pending: Map<number, resolver>;

    constructor() {
        this.next = 1;
        this.pending = new Map<number, resolver>();
    }

    async waitForTurn(turn: number): Promise<boolean> {
        if (turn == 0) {
            return true;
        } else if (turn == this.next) {
            this.next++;
            this.flushPending();
            return true;
        } else if (turn > this.next) {
            let resolver: resolver;
            let task = new Promise<boolean>((resolve, _reject) => {
                resolver = resolve;
            });
            this.pending.set(turn, resolver);
            return task;
        } else {
            return false;
        }
    }

    private flushPending(): void {
        while (this.pending.has(this.next)) {
            let resolver = this.pending.get(this.next);
            this.pending.delete(this.next);
            resolver();
            this.next++;
        }
    } 
}