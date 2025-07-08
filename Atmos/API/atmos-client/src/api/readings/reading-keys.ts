export const readingKeys = {
  all: ["readings"],
  single: (id: string) => ["readings", id] as const,
};
