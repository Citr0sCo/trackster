import { IStack } from './stack.interface';

export class Stack<T> implements IStack<T> {

    private _storage: Array<T> = new Array<T>();
    private _capacity: number = Infinity;

    public push(item: T): void {
        if (this.size() === this._capacity) {
            throw Error('Stack has reached max capacity, you cannot add more items');
        }
        this._storage.push(item);
    }

    public pop(): T | undefined {
        return this._storage.pop();
    }

    public peek(): T | undefined {
        return this._storage[this.size() - 1];
    }

    public size(): number {
        return this._storage.length;
    }
}